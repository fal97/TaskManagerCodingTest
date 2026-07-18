import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import {
  CreateUserTaskRequest,
  TaskPriority,
  UserTask,
} from '../../../models';

export interface TaskFormDialogData {
  task?: UserTask;
}

@Component({
  selector: 'app-task-form-dialog',
  imports: [
    MatButtonModule,
    MatDatepickerModule,
    MatDialogActions,
    MatDialogClose,
    MatDialogContent,
    MatDialogTitle,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    MatSelectModule,
    ReactiveFormsModule,
  ],
  templateUrl: './task-form-dialog.html',
  styleUrl: './task-form-dialog.css',
})
export class TaskFormDialog {
  private readonly formBuilder = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<TaskFormDialog>);
  protected readonly data = inject<TaskFormDialogData>(MAT_DIALOG_DATA);
  protected readonly minDueDate = new Date();

  protected readonly priorities = [
    { value: TaskPriority.Low, label: 'Low' },
    { value: TaskPriority.Normal, label: 'Normal' },
    { value: TaskPriority.High, label: 'High' },
    { value: TaskPriority.Critical, label: 'Critical' },
  ];

  protected readonly form = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(2000)],
    priority: [TaskPriority.Normal, Validators.required],
    dueDate: [null as Date | null],
    notes: ['', Validators.maxLength(2000)],
  });

  constructor() {
    const task = this.data.task;

    if (task) {
      this.form.patchValue({
        title: task.title,
        description: task.description ?? '',
        priority: task.priority,
        dueDate: task.dueDate ? new Date(task.dueDate) : null,
        notes: task.notes ?? '',
      });
    }
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: CreateUserTaskRequest = {
      title: value.title?.trim() ?? '',
      description: value.description?.trim() || null,
      priority: value.priority ?? TaskPriority.Normal,
      dueDate: value.dueDate?.toISOString() ?? null,
      notes: value.notes?.trim() || null,
    };

    this.dialogRef.close(request);
  }
}
