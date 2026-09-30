import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Specialisation } from '../../core/models/specialisation.model';
import { SpecialisationsService } from '../../core/services/specialisations.service';

@Component({
  selector: 'app-specialisations',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTableModule,
  ],
  templateUrl: './specialisations.component.html',
  styleUrl: './specialisations.component.css',
})
export class SpecialisationsComponent {
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly editing = signal(false);
  readonly specialisations = signal<Specialisation[]>([]);
  readonly editingId = signal<string | null>(null);
  readonly displayedColumns = ['name', 'doctors', 'actions'];
  readonly form;

  constructor(
    private readonly fb: FormBuilder,
    private readonly service: SpecialisationsService,
    private readonly snackBar: MatSnackBar,
  ) {
    this.form = this.fb.nonNullable.group({ name: ['', Validators.required] });
    this.load();
  }

  beginCreate(): void {
    this.editingId.set(null);
    this.form.reset({ name: '' });
    this.editing.set(true);
  }

  beginEdit(item: Specialisation): void {
    this.editingId.set(item.id);
    this.form.reset({ name: item.name });
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.editingId.set(null);
    this.form.reset();
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const id = this.editingId();
    const request = { name: this.form.controls.name.value.trim() };
    if (!request.name) {
      this.form.controls.name.setErrors({ required: true });
      return;
    }

    this.saving.set(true);
    const operation = id ? this.service.update(id, request) : this.service.create(request);
    operation.subscribe({
      next: (saved) => {
        this.specialisations.update((items) =>
          id ? items.map((item) => (item.id === saved.id ? saved : item)) : [...items, saved],
        );
        this.saving.set(false);
        this.cancelEdit();
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Could not save the specialisation.', 'Dismiss', { duration: 4000 });
      },
    });
  }

  remove(item: Specialisation): void {
    if (!confirm(`Delete specialisation "${item.name}"?`)) {
      return;
    }

    this.service.delete(item.id).subscribe({
      next: () => {
        this.specialisations.update((items) => items.filter((current) => current.id !== item.id));
        this.snackBar.open('Specialisation deleted.', 'OK', { duration: 3000 });
      },
      error: () =>
        this.snackBar.open('Could not delete the specialisation.', 'Dismiss', { duration: 4000 }),
    });
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: (items) => {
        this.specialisations.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load specialisations.', 'Dismiss', { duration: 4000 });
      },
    });
  }
}
