import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { Observable, finalize } from 'rxjs';

import { ConfirmationDialog } from '../../../../shared/components';
import {
  TaskPriority,
  TaskStatus,
  UserTask,
} from '../../models';
import {
  SortDirection,
  UserTaskFilters,
  UserTaskService,
  UserTaskSortField,
} from '../../services';
import {
  TaskFormDialog,
  TaskFormDialogData,
  TaskFormResult,
} from './task-form-dialog/task-form-dialog';

@Component({
  selector: 'app-task-list',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSortModule,
    MatTableModule,
    ReactiveFormsModule,
  ],
  templateUrl: './task-list.html',
  styleUrl: './task-list.css',
})
export class TaskList {
  private readonly userTaskService = inject(UserTaskService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly formBuilder = inject(FormBuilder);
  private currentFilters: UserTaskFilters = {};

  protected readonly tasks = signal<UserTask[]>([]);
  protected readonly loading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly actionTaskId = signal<number | null>(null);
  protected readonly activeSort = signal<UserTaskSortField>('createdDate');
  protected readonly activeSortDirection = signal<SortDirection>('desc');
  protected readonly completedStatus = TaskStatus.Completed;
  protected readonly displayedColumns = ['title', 'status', 'priority', 'dueDate', 'actions'];

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

  protected readonly filterForm = this.formBuilder.group({
    searchTerm: [''],
    status: [null as TaskStatus | null],
    priority: [null as TaskPriority | null],
    dueFrom: [null as Date | null],
    dueTo: [null as Date | null],
  });

  constructor() {
    this.loadTasks();
  }

  protected openCreateDialog(): void {
    this.dialog
      .open<TaskFormDialog, TaskFormDialogData, TaskFormResult>(TaskFormDialog, {
        data: {},
        disableClose: true,
      })
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((request) => {
        if (request) {
          this.runMutation(this.userTaskService.create(request), 'Task created.');
        }
      });
  }

  protected openEditDialog(task: UserTask): void {
    this.dialog
      .open<TaskFormDialog, TaskFormDialogData, TaskFormResult>(TaskFormDialog, {
        data: { task },
        disableClose: true,
      })
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((request) => {
        if (request) {
          this.runMutation(
            this.userTaskService.update({ id: task.id, ...request }),
            'Task updated.',
            task.id,
          );
        }
      });
  }

  protected markCompleted(task: UserTask): void {
    this.runMutation(
      this.userTaskService.update({ id: task.id, status: TaskStatus.Completed }),
      'Task marked as completed.',
      task.id,
    );
  }

  protected confirmDelete(task: UserTask): void {
    this.dialog
      .open(ConfirmationDialog, {
        data: {
          title: 'Delete task?',
          message: `“${task.title}” will be removed from your task list.`,
          confirmLabel: 'Delete',
        },
      })
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed) => {
        if (confirmed) {
          this.runMutation(this.userTaskService.delete(task.id), 'Task deleted.', task.id);
        }
      });
  }

  protected applyFilters(): void {
    const value = this.filterForm.getRawValue();

    this.loadTasks({
      searchTerm: value.searchTerm?.trim() || undefined,
      status: value.status ?? undefined,
      priority: value.priority ?? undefined,
      dueFrom: value.dueFrom?.toISOString(),
      dueTo: value.dueTo ? this.toEndOfDayIso(value.dueTo) : undefined,
      sortBy: this.activeSort(),
      sortDirection: this.activeSortDirection(),
    });
  }

  protected clearFilters(): void {
    this.filterForm.reset({
      searchTerm: '',
      status: null,
      priority: null,
      dueFrom: null,
      dueTo: null,
    });
    this.activeSort.set('createdDate');
    this.activeSortDirection.set('desc');
    this.loadTasks({ sortBy: 'createdDate', sortDirection: 'desc' });
  }

  protected sortTable(sort: Sort): void {
    if (!sort.direction) {
      return;
    }

    this.activeSort.set(sort.active as UserTaskSortField);
    this.activeSortDirection.set(sort.direction);
    this.applyFilters();
  }

  protected loadTasks(filters: UserTaskFilters = this.currentFilters): void {
    this.currentFilters = filters;
    this.loading.set(true);
    this.errorMessage.set(null);

    this.userTaskService
      .search(filters)
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

  private toEndOfDayIso(date: Date): string {
    const endOfDay = new Date(date);
    endOfDay.setHours(23, 59, 59, 999);
    return endOfDay.toISOString();
  }

  private runMutation(
    operation: Observable<UserTask | void>,
    successMessage: string,
    taskId: number | null = null,
  ): void {
    this.actionTaskId.set(taskId);

    operation
      .pipe(
        finalize(() => this.actionTaskId.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.snackBar.open(successMessage, 'Dismiss', { duration: 3000 });
          this.loadTasks();
        },
        error: () => {
          this.snackBar.open('The task could not be saved. Please try again.', 'Dismiss', {
            duration: 5000,
          });
        },
      });
  }
}
