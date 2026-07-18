import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';

import { TaskPriority, TaskStatus, UserTask } from '../../models';
import { UserTaskFilters, UserTaskService } from '../../services';

@Component({
  selector: 'app-task-list',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule,
  ],
  templateUrl: './task-list.html',
  styleUrl: './task-list.css',
})
export class TaskList {
  private readonly userTaskService = inject(UserTaskService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly tasks = signal<UserTask[]>([]);
  protected readonly loading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly displayedColumns = ['title', 'status', 'priority', 'dueDate'];

  protected readonly statuses = [
    { value: TaskStatus.Pending, label: 'Pending' },
    { value: TaskStatus.InProgress, label: 'In progress' },
    { value: TaskStatus.Completed, label: 'Completed' },
    { value: TaskStatus.Cancelled, label: 'Cancelled' },
    { value: TaskStatus.OnHold, label: 'On hold' },
  ];

  protected readonly priorities = [
    { value: TaskPriority.Low, label: 'Low' },
    { value: TaskPriority.Normal, label: 'Normal' },
    { value: TaskPriority.High, label: 'High' },
    { value: TaskPriority.Critical, label: 'Critical' },
  ];

  constructor() {
    this.loadTasks();
  }

  protected applyFilters(status: TaskStatus | '', priority: TaskPriority | ''): void {
    this.loadTasks({
      status: status === '' ? undefined : status,
      priority: priority === '' ? undefined : priority,
    });
  }

  protected loadTasks(filters: UserTaskFilters = {}): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.userTaskService
      .getAll(filters)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (tasks) => {
          this.tasks.set(tasks);
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Tasks could not be loaded. Please try again.');
          this.loading.set(false);
        },
      });
  }

  protected statusLabel(status: TaskStatus): string {
    return this.statuses.find((option) => option.value === status)?.label ?? 'Unknown';
  }

  protected priorityLabel(priority: TaskPriority): string {
    return this.priorities.find((option) => option.value === priority)?.label ?? 'Unknown';
  }

  protected statusClass(status: TaskStatus): string {
    return `status-${TaskStatus[status]?.toLowerCase() ?? 'unknown'}`;
  }

  protected priorityClass(priority: TaskPriority): string {
    return `priority-${TaskPriority[priority]?.toLowerCase() ?? 'unknown'}`;
  }
}
