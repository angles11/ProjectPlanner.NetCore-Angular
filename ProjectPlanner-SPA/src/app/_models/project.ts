import { User } from './user';
import { Todo } from './todo';

export interface Project {
    id: number;
    title: string;
    shortDescription: string;
    longDescription: string;
    created: Date;
    estimatedDate: Date;
    modified: Date;
    owner: User;
    ownerId: string;
    collaborators: User[];
    todos: Todo[];
}
