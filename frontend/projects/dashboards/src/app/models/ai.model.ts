export interface MessagesHistory {
  dashboardId: number;
  messages: Message[];
}

export interface Message {
  text: string;
  isUser: boolean;
  timestamp: Date;
  isNotification: boolean;
}
