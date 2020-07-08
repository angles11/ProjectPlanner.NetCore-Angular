import { User } from './user';

export interface Friend {
    userFriend: User;
    since: Date;
    status: any;
    sentById: string;
}
