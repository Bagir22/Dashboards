import { Component, ViewChild, ElementRef, inject, Input, Output, EventEmitter, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { marked } from 'marked';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { MetabaseService } from '../metabase/metabase.service';
import { lastValueFrom } from 'rxjs';
import { TabCard, TabInfo } from '../metabase/metabase.model';

export interface Message {
  text: string;
  isUser: boolean;
  timestamp: Date;
  isNotification: boolean;
}

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ai-assistant.component.html',
  styleUrls: ['./ai-assistant.component.scss']
})
export class AiAssistantComponent implements OnChanges, OnInit {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  
  @Input() currentDashboardName: string | null = null;
  @Input() currentDashboardId: number | null = null;
  @Input() baseUrl: string = "";
  @Output() close = new EventEmitter<void>();

  messages: Message[] = [];
  
  private readonly STORAGE_KEY = 'ai_assistant_messages';
  private readonly MAX_STORED_MESSAGES = 100;

  isLoading = false;
  currentStreamingMessage = '';
  private abortController: AbortController | null = null;
  protected groupedCardsbyTabs?: TabInfo[]; 
  protected copySuccess: number | null = null;

  private sanitizer = inject(DomSanitizer);
  private metabaseService = inject(MetabaseService);
  private apiAnalysisUrl = 'http://localhost:8080/api/analysis/';

  constructor() {
    marked.setOptions({
      breaks: true,
      gfm: true,
    });
  }

  ngOnInit() {
    this.loadMessagesFromStorage();
  }

  public ngOnChanges(changes: SimpleChanges): void {
    const dashboardIdChanges = changes['currentDashboardId'];
    
    if (dashboardIdChanges?.currentValue !== dashboardIdChanges?.previousValue) {
      this.getCards();
    }
  }

  async copyToClipboard(text: string, messageIndex?: number): Promise<void> {
    try {
      await navigator.clipboard.writeText(text);
      
      if (messageIndex !== undefined) {
        this.copySuccess = messageIndex;
        setTimeout(() => {
          this.copySuccess = null;
        }, 2000);
      }
    } catch (err) {
      console.error('Ошибка при копировании:', err);
    }
  }

  /**
 * Проверка, нужно ли показывать разделитель даты перед сообщением
 */
shouldShowDateSeparator(index: number): boolean {
  if (index === 0) return true;
  
  const currentDate = new Date(this.messages[index].timestamp).toDateString();
  const previousDate = new Date(this.messages[index - 1].timestamp).toDateString();
  
  return currentDate !== previousDate;
}

  /**
   * Загрузка сообщений из localStorage
   */
  private loadMessagesFromStorage(): void {
    try {
      const savedMessages = localStorage.getItem(this.STORAGE_KEY);
      if (savedMessages) {
        const parsedMessages = JSON.parse(savedMessages);
        this.messages = parsedMessages.map((msg: any) => ({
          ...msg,
          timestamp: new Date(msg.timestamp)
        }));
      } else {
        this.addWelcomeMessage();
      }
    } catch (error) {
      console.error('Ошибка при загрузке истории сообщений:', error);
      this.addWelcomeMessage();
    }
  }

  /**
   * Добавление приветственного сообщения
   */
  private addWelcomeMessage(): void {
    this.messages = [{
      text: 'Здравствуйте! Я ИИ-помощник. Выберите действие для анализа данных текущего дашборда.',
      isUser: false,
      timestamp: new Date(),
      isNotification: true,
    }];
    this.saveMessagesToStorage();
  }

  /**
   * Сохранение сообщений в localStorage
   */
  private saveMessagesToStorage(): void {
    try {
      const messagesToSave = this.messages.slice(-this.MAX_STORED_MESSAGES);
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(messagesToSave));
    } catch (error) {
      console.error('Ошибка при сохранении истории сообщений:', error);
    }
  }

  /**
   * Очистка истории сообщений
   */
  clearHistory(): void {
    if (confirm('Очистить историю сообщений?')) {
      localStorage.removeItem(this.STORAGE_KEY);
      this.addWelcomeMessage();
      this.scrollToBottom();
    }
  }

  private async getCards() {
    if (!this.currentDashboardId) return;
    const authData = this.metabaseService.getAuthCredentials("admin@example.com", "Admin123Qwerty");
    await lastValueFrom(await this.metabaseService.getGroupedCardsByTabs(this.baseUrl, this.currentDashboardId, authData)).then(
      response => this.groupedCardsbyTabs = Object.values(response)
    );
  }

  renderMarkdown(text: string): SafeHtml {
    if (!text) return 'Думаю...';
  
    const html = marked.parse(text) as string;
    
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

// Анализ карточки (графика)
async executeCardAction(card: TabCard): Promise<void> {
  if (this.shouldBlockAction()) return;

  this.addUserMessage(card.name);
  const assistantMessage = this.addAssistantMessage();
  
  this.setLoadingState(true);
  
  this.abortController = new AbortController();

  try {
    await this.streamCardResponse(card.id);
  } catch (error) {
    await this.handleStreamingError(error);
  } finally {
    this.resetLoadingState();
  }
}

// Анализ таба (графиков)
async executeTabAction(tab: TabInfo): Promise<void> {
  if (this.shouldBlockAction()) return;

  this.addUserMessage(tab.name);
  const assistantMessage = this.addAssistantMessage();
  
  this.setLoadingState(true);
  
  this.abortController = new AbortController();

  try {
    const cardIds = tab.cards.map(card => card.id);
    await this.streamTabResponse(cardIds);
  } catch (error) {
    await this.handleStreamingError(error);
  } finally {
    this.resetLoadingState();
  }
}

// Проверка возможности выполнения действия
private shouldBlockAction(): boolean {
  return this.isLoading || !this.currentDashboardId;
}

// Добавление сообщения пользователя
private addUserMessage(analyzed: string): void {
  const text = `Проанализируй ${analyzed}`;
  
  this.messages.push({
    text,
    isUser: true,
    timestamp: new Date(),
    isNotification: true,
  });
  
  this.saveMessagesToStorage();
}

// Добавление сообщения ассистента (пустого, для стриминга)
private addAssistantMessage(): Message {
  const assistantMessage: Message = {
    text: '',
    isUser: false,
    timestamp: new Date(),
    isNotification: false,
  };
  
  this.messages.push(assistantMessage);
  return assistantMessage;
}

// Установка состояния загрузки
private setLoadingState(isLoading: boolean): void {
  this.isLoading = isLoading;
  this.currentStreamingMessage = '';
  this.scrollToBottom();
}

// Сброс состояния загрузки
private resetLoadingState(): void {
  this.isLoading = false;
  this.currentStreamingMessage = '';
  this.abortController = null;
  this.scrollToBottom();
}

// Основной метод стриминга ответа
private async streamCardResponse(cardId: number): Promise<void> {
  const response = await this.makeCardApiRequest(cardId);
  await this.processStream(response);
  this.saveMessagesToStorage();
}

private async streamTabResponse(cardIds: number[]): Promise<void> {
  const response = await this.makeCardsApiRequest(cardIds);
  await this.processStream(response);
  this.saveMessagesToStorage();
}

// Выполнение API запроса
private async makeCardApiRequest(cardId: number): Promise<Response> {
  const response = await fetch(this.apiAnalysisUrl + 'card', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(cardId),
    signal: this.abortController?.signal
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  if (!response.body) {
    throw new Error('Response body is null');
  }

  return response;
}

private async makeCardsApiRequest(cardIds: number[]): Promise<Response> {
  const response = await fetch(this.apiAnalysisUrl + 'cards', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(cardIds),
    signal: this.abortController?.signal
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  if (!response.body) {
    throw new Error('Response body is null');
  }

  return response;
}

// Обработка стрима
private async processStream(response: Response): Promise<void> {
  const reader = response.body!.getReader();
  const decoder = new TextDecoder();
  let buffer = '';

  while (true) {
    const { value, done } = await reader.read();
    
    if (done) {
      break;
    }

    buffer = this.processChunk(value, buffer, decoder);
  }
}

// Обработка одного чанка данных
private processChunk(value: Uint8Array, buffer: string, decoder: TextDecoder): string {
  buffer += decoder.decode(value, { stream: true });
  
  const messages = buffer.split('\n\n');
  const newBuffer = messages.pop() || '';

  for (const message of messages) {
    this.processStreamMessage(message);
  }

  return newBuffer;
}

// Обработка одного сообщения из стрима
private processStreamMessage(message: string): void {
  if (!message.startsWith('data: ')) return;

  const content = message.slice(6);
  
  if (content === '[DONE]') {
    return;
  } else if (content.startsWith('Error: ')) {
    throw new Error(content.slice(7));
  } else {
    this.updateStreamingContent(content);
  }
}

// Обновление контента при стриминге
private updateStreamingContent(content: string): void {
  this.currentStreamingMessage += content;
  
  const lastMessage = this.messages[this.messages.length - 1];
  if (!lastMessage.isUser) {
    lastMessage.text = this.currentStreamingMessage;
  }
  
  this.scrollToBottom();
}

// Обработка ошибок стриминга
private async handleStreamingError(error: any): Promise<void> {
  if (error.name === 'AbortError') {
    this.addAbortMessage();
  } else {
    this.handleGeneralError(error);
  }
  
  await this.saveMessagesToStorage();
}

// Добавление сообщения об отмене запроса
private addAbortMessage(): void {
  this.messages.push({
    text: 'Запрос был отменен.',
    isUser: false,
    timestamp: new Date(),
    isNotification: true,
  });
}

// Обработка общей ошибки
private handleGeneralError(error: any): void {
  console.error('Streaming Error:', error);
  
  // Удаляем последнее сообщение (пустое сообщение ассистента)
  this.messages.pop();
  
  const errorMessage = this.formatErrorMessage(error);
  
  this.messages.push({
    text: errorMessage,
    isUser: false,
    timestamp: new Date(),
    isNotification: true,
  });
}

// Форматирование сообщения об ошибке
private formatErrorMessage(error: any): string {
  if (error.message) {
    return `Ошибка: ${error.message}`;
  }
  
  return 'Произошла ошибка при анализе данных. Пожалуйста, попробуйте позже.';
}

// Отмена запроса
cancelRequest(): void {
  if (this.abortController) {
    this.abortController.abort();
    this.abortController = null;
  }
}

  private scrollToBottom(): void {
    setTimeout(() => {
      if (this.messagesContainer) {
        this.messagesContainer.nativeElement.scrollTop = 
          this.messagesContainer.nativeElement.scrollHeight;
      }
    });
  }

  /**
   * Получение отформатированной даты для отображения
   */
  formatMessageDate(timestamp: Date): string {
    const date = new Date(timestamp);
    const today = new Date();
    const yesterday = new Date(today);
    yesterday.setDate(yesterday.getDate() - 1);

    if (date.toDateString() === today.toDateString()) {
      return 'Сегодня ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
    } else if (date.toDateString() === yesterday.toDateString()) {
      return 'Вчера ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
    } else {
      return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
    }
  }
}