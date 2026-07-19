import { IsoDateString } from './iso-date-string.type';
import { TaskPriority } from './task-priority.enum';
import { TaskStatus } from './task-status.enum';

/** Matches the backend UserTaskResponse DTO. */
export interface UserTask {
  id: number;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: IsoDateString | null;
  completedDate: IsoDateString | null;
  notes: string | null;
  createdDate: IsoDateString;
  lastModifiedDate: IsoDateString;
  isDeleted: boolean;
}
