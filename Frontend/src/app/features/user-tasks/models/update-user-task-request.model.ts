import { IsoDateString } from './iso-date-string.type';
import { TaskPriority } from './task-priority.enum';
import { TaskStatus } from './task-status.enum';

/** Matches the backend UpdateUserTaskRequest DTO. */
export interface UpdateUserTaskRequest {
  id: number;
  title?: string | null;
  description?: string | null;
  status?: TaskStatus | null;
  priority?: TaskPriority | null;
  dueDate?: IsoDateString | null;
  notes?: string | null;
}
