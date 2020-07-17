import { User } from './user';

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
}
