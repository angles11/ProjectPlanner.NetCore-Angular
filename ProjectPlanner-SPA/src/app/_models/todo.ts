import { Project } from './project';
import { TodoMessage } from './todoMessage';

export interface Todo {

    id: number;
    title: string;
    shortDescription: string;
    longDescription: string;
    created: Date;
    modified: Date;
    estimatedDate: Date;
    projectId: number;
    project: Project;
    status: string;
    messages: TodoMessage[];
}
