import { Component, ViewChild, ElementRef, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
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

  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:8080/api/ai-assistant';

  async executeAction(action: any): Promise<void> {
    if (this.isLoading || !this.currentDashboardId) return;

    this.messages.push({
      text: action.text,
      isUser: true,
      timestamp: new Date()
    });

    this.isLoading = true;
    this.scrollToBottom();

    try {
      const response = await firstValueFrom(
        this.http.post<any>(`${this.apiUrl}/${action.endpoint}`, {
          dashboardId: this.currentDashboardId,
          dashboardName: this.currentDashboardName,
          actionId: action.id
        })
      );

      this.messages.push({
        text: response.message || response.analysis || 'Анализ выполнен успешно.',
        isUser: false,
        timestamp: new Date()
      });

    } catch (error) {
      console.error('API Error:', error);
      
      let errorMessage = 'Произошла ошибка при анализе данных. Пожалуйста, попробуйте позже.';
      
      if (error instanceof Error) {
        errorMessage = `Ошибка: ${error.message}`;
      }

      this.messages.push({
        text: errorMessage,
        isUser: false,
        timestamp: new Date()
      });
    } finally {
      this.isLoading = false;
      this.scrollToBottom();
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