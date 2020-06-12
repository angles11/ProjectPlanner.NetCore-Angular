import { User } from './user';

export interface Friendship {
  senderId: string;
  sender: User;
  recipientId: string;
  recipient: User;
  since: Date;
}
