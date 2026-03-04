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
  template: `
    <div class="ai-assistant">
      <!-- Заголовок -->
      <div class="ai-header">
        <div class="header-left">
          <h3>ИИ-помощник</h3>
          <span class="dashboard-indicator" *ngIf="currentDashboardName" [title]="'Анализ дашборда: ' + currentDashboardName">
          </span>
        </div>
      </div>
      
      <!-- Сообщения -->
      <div class="messages-container" #messagesContainer>
        <div *ngFor="let msg of messages" class="message" [class.user-message]="msg.isUser">
          <div class="message-content-wrapper">
            <div class="message-content">{{ msg.text }}</div>
            <div class="message-time">{{ msg.timestamp | date:'HH:mm' }}</div>
          </div>
        </div>
        
        <!-- Индикатор загрузки -->
        <div *ngIf="isLoading" class="loading-indicator">
          <tui-loader size="s" [inheritColor]="false"></tui-loader>
          <span>Анализирую данные...</span>
        </div>
      </div>

      <!-- Кнопки действий -->
      <div class="actions-grid">
        <button
          *ngFor="let action of actions"
          class="action-btn"
          [class.primary]="action.primary"
          [disabled]="isLoading"
          (click)="executeAction(action)">
          <span class="action-icon">{{ action.icon }}</span>
          <span class="action-text">{{ action.text }}</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .ai-assistant {
      height: 100%;
      display: flex;
      flex-direction: column;
      background: white;
      border-radius: 0.75rem;
      overflow: hidden;
      box-shadow: 0 4px 20px rgba(0,0,0,0.15);
    }

    .ai-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.25rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      
      .header-left {
        display: flex;
        align-items: center;
        gap: 0.5rem;
      }

      .ai-icon {
        font-size: 1.25rem;
      }

      h3 {
        margin: 0;
        font-size: 1rem;
        font-weight: 600;
      }

      .dashboard-indicator {
        font-size: 1rem;
        cursor: help;
      }
    }

    .messages-container {
      flex: 1;
      overflow-y: auto;
      padding: 1.25rem;
      display: flex;
      flex-direction: column;
      gap: 1rem;
      min-height: 300px;
      max-height: 400px;
      background: #f8fafc;
    }

    .message {
      display: flex;
      gap: 0.75rem;
      max-width: 90%;
    }

    .user-message {
      align-self: flex-end;
      flex-direction: row-reverse;
    }

    .message-avatar {
      width: 2rem;
      height: 2rem;
      border-radius: 50%;
      background: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      flex-shrink: 0;
    }

    .message-content-wrapper {
      background: white;
      padding: 0.75rem 1rem;
      border-radius: 1rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    }

    .user-message .message-content-wrapper {
      background: #667eea;
      color: white;
    }

    .message-content {
      font-size: 0.875rem;
      line-height: 1.5;
      word-break: break-word;
      white-space: pre-wrap;
    }

    .message-time {
      font-size: 0.625rem;
      opacity: 0.6;
      margin-top: 0.25rem;
      text-align: right;
    }

    .loading-indicator {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1rem;
      background: white;
      border-radius: 1rem;
      color: #64748b;
      font-size: 0.875rem;
    }

    .actions-grid {
      display: grid;
      gap: 0.75rem;
      padding: 1rem;
      background: white;
      border-top: 1px solid #e2e8f0;
    }

    .action-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #e2e8f0;
      border-radius: 0.5rem;
      background: white;
      color: #334155;
      font-size: 0.875rem;
      cursor: pointer;
      transition: all 0.2s;

      &:hover:not(:disabled) {
        background: #f8fafc;
        border-color: #94a3b8;
      }

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      &.primary {
        color: #334155;

        &:hover:not(:disabled) {
          background: #f8fafc;
          border-color: #94a3b8;
        }
      }

      .action-icon {
        font-size: 1.1rem;
      }
      
      .action-text {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }
  `]
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
  private apiUrl = 'http://localhost:8080/api/ai-assistant'; // URL вашего C# API

  async executeAction(action: any): Promise<void> {
    if (this.isLoading || !this.currentDashboardId) return;

    // Добавляем сообщение пользователя
    this.messages.push({
      text: action.text,
      isUser: true,
      timestamp: new Date()
    });

    this.isLoading = true;
    this.scrollToBottom();

    try {
      // Реальный запрос к C# API
      const response = await firstValueFrom(
        this.http.post<any>(`${this.apiUrl}/${action.endpoint}`, {
          dashboardId: this.currentDashboardId,
          dashboardName: this.currentDashboardName,
          actionId: action.id
        })
      );

      // Добавляем ответ от API
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