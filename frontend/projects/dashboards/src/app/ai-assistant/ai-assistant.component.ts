import { Component, ViewChild, ElementRef, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { TuiLoader } from '@taiga-ui/core';

export interface Message {
  text: string;
  isUser: boolean;
  timestamp: Date;
}

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule, TuiLoader],
  templateUrl: './ai-assistant.component.html',
  styleUrls: ['./ai-assistant.component.scss']
})
export class AiAssistantComponent {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  
  @Input() currentDashboardName: string | null = null;
  @Input() currentDashboardId: string | null = null;
  @Output() close = new EventEmitter<void>();

  messages: Message[] = [
    {
      text: 'Здравствуйте! Я ИИ-помощник. Выберите действие для анализа данных текущего дашборда.',
      isUser: false,
      timestamp: new Date()
    }
  ];

  actions = [
    { id: 'overview', text: 'Анализ дашборда', icon: '📊', endpoint: 'overview', primary: true },
    { id: 'trends', text: 'Промпт 1', icon: '📊', endpoint: 'trends', primary: false },
    { id: 'statistics', text: 'Промпт 2', icon: '📊', endpoint: 'statistics', primary: false },
    { id: 'insights', text: 'Промпт 3', icon: '📊', endpoint: 'insights', primary: false }
  ];

  isLoading = false;
  currentStreamingMessage = '';
  private abortController: AbortController | null = null;

  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:8080/api/analysis/stream';

  async executeAction(action: any): Promise<void> {
    if (this.isLoading || !this.currentDashboardId) return;

    // Добавляем сообщение пользователя
    this.messages.push({
      text: action.text,
      isUser: true,
      timestamp: new Date()
    });

    // Создаем пустое сообщение для ассистента, которое будем заполнять
    const assistantMessage: Message = {
      text: '',
      isUser: false,
      timestamp: new Date()
    };
    this.messages.push(assistantMessage);
    
    this.isLoading = true;
    this.currentStreamingMessage = '';
    this.scrollToBottom();

    // Создаем AbortController для возможности отмены запроса
    this.abortController = new AbortController();

    try {
      // Формируем запрос для стриминга
      const response = await fetch(this.apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(this.getUserRequest(action)),
        signal: this.abortController.signal
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      if (!response.body) {
        throw new Error('Response body is null');
      }

      const reader = response.body.getReader();
      const decoder = new TextDecoder();
      let buffer = '';

      while (true) {
        const { value, done } = await reader.read();
        
        if (done) {
          break;
        }

        buffer += decoder.decode(value, { stream: true });
        
        // Разбираем буфер на сообщения SSE
        const messages = buffer.split('\n\n');
        buffer = messages.pop() || '';

        for (const message of messages) {
          if (message.startsWith('data: ')) {
            const content = message.slice(6);
            
            if (content === '[DONE]') {
              continue;
            } else if (content.startsWith('Error: ')) {
              throw new Error(content.slice(7));
            } else {
              this.currentStreamingMessage += content;
              
              const lastMessage = this.messages[this.messages.length - 1];
              if (!lastMessage.isUser) {
                lastMessage.text = this.currentStreamingMessage;
              }
              
              this.scrollToBottom();
            }
          }
        }
      }

    } catch (error: any) {
      // Проверяем, не была ли это отмена запроса
      if (error.name === 'AbortError') {
        // Добавляем сообщение об отмене
        this.messages.push({
          text: 'Запрос был отменен.',
          isUser: false,
          timestamp: new Date()
        });
      } else {
        console.error('Streaming Error:', error);
        
        // Убираем пустое сообщение ассистента если оно было добавлено
        this.messages.pop();
        
        let errorMessage = 'Произошла ошибка при анализе данных. Пожалуйста, попробуйте позже.';
        
        if (error.message) {
          errorMessage = `Ошибка: ${error.message}`;
        }

        this.messages.push({
          text: errorMessage,
          isUser: false,
          timestamp: new Date()
        });
      }
    } finally {
      this.isLoading = false;
      this.currentStreamingMessage = '';
      this.abortController = null;
      this.scrollToBottom();
    }
  }

  private getUserRequest(action: any): string {
    return `Проанализируй дашборд "${this.currentDashboardName}" с помощью действия "${action.text}"`;
  }

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
}