import { IsoDateString } from './iso-date-string.type';
import { TaskPriority } from './task-priority.enum';

/** Matches the backend CreateUserTaskRequest DTO. */
export interface CreateUserTaskRequest {
  title: string;
  description?: string | null;
  priority: TaskPriority;
  dueDate?: IsoDateString | null;
  notes?: string | null;
}
