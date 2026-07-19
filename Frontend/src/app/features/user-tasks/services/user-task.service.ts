import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  CreateUserTaskRequest,
  IsoDateString,
  TaskPriority,
  TaskStatus,
  UpdateUserTaskRequest,
  UserTask,
} from '../models';

export interface UserTaskFilters {
  searchTerm?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueFrom?: IsoDateString;
  dueTo?: IsoDateString;
  sortBy?: UserTaskSortField;
  sortDirection?: SortDirection;
}

export type UserTaskSortField =
  | 'title'
  | 'status'
  | 'priority'
  | 'dueDate'
  | 'createdDate'
  | 'lastModifiedDate';

export type SortDirection = 'asc' | 'desc';

@Injectable({ providedIn: 'root' })
export class UserTaskService {
  private readonly http = inject(HttpClient);
  private readonly resourceUrl = `${environment.apiBaseUrl}/api/tasks`;

  getAll(filters: UserTaskFilters = {}): Observable<UserTask[]> {
    let params = new HttpParams();

    if (filters.status !== undefined) {
      params = params.set('status', filters.status);
    }

    if (filters.priority !== undefined) {
      params = params.set('priority', filters.priority);
    }

    return this.http.get<UserTask[]>(this.resourceUrl, { params });
  }

  search(filters: UserTaskFilters = {}): Observable<UserTask[]> {
    let params = new HttpParams();

    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== '') {
        params = params.set(key, value);
      }
    });

    return this.http.get<UserTask[]>(`${this.resourceUrl}/search`, { params });
  }

  getById(id: number): Observable<UserTask> {
    return this.http.get<UserTask>(`${this.resourceUrl}/${id}`);
  }

  create(request: CreateUserTaskRequest): Observable<UserTask> {
    return this.http.post<UserTask>(this.resourceUrl, request);
  }

  update(request: UpdateUserTaskRequest): Observable<UserTask> {
    return this.http.put<UserTask>(`${this.resourceUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.resourceUrl}/${id}`);
  }
}
