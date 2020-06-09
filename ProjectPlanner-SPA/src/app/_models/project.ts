export interface Project {
  id: number;
  title: string;
  description: string;
  created: Date;
  modified: Date;
  estimatedDate: Date;
  ownerId: string;
  collaborators: [];
}
