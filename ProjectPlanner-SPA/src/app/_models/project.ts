import { User } from './user';
import { Todo } from './todo';
import { TodoMessage } from './todoMessage';

export interface Project {
    id: number;
    title: string;
    shortDescription: string;
    longDescription: string;
    created: Date;
    estimatedDate: Date;
    modified: Date;
    owner: User;
    completedPercentage: number;
    ownerId: string;
    collaborators: User[];
    todos: Todo[];
    lastMessage: TodoMessage;
}
