import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DoctorsService } from '../../core/services/doctors.service';
import { SpecialisationsService } from '../../core/services/specialisations.service';
import { Doctor } from '../../core/models/doctor.model';
import { Specialisation } from '../../core/models/specialisation.model';
import { employeeWorkStatusLabels } from '../../core/models/enums';
import { DoctorDialogComponent } from './doctor-dialog/doctor-dialog.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './doctors.component.html',
  styleUrl: './doctors.component.css',
})
export class DoctorsComponent {
  readonly loading = signal(true);
  readonly doctors = signal<Doctor[]>([]);
  readonly specialisations = signal<Specialisation[]>([]);
  readonly workStatusLabels = employeeWorkStatusLabels;
  readonly displayedColumns = ['fullName', 'login', 'specialisations', 'status', 'actions'];

  constructor(
    private readonly doctorsService: DoctorsService,
    private readonly specialisationsService: SpecialisationsService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
    protected readonly auth: AuthService,
  ) {
    this.load();
  }

  specialisationNames(doctor: Doctor): string[] {
    const byId = new Map(this.specialisations().map((s) => [s.entityId, s.name]));
    return doctor.specialisations.map((id) => byId.get(id) ?? id);
  }

  workStatusLabel(doctor: Doctor): string {
    return this.workStatusLabels[doctor.employeeWorkStatus];
  }

  edit(doctor: Doctor): void {
    this.dialog
      .open(DoctorDialogComponent, {
        width: '480px',
        data: { doctor, specialisations: this.specialisations() },
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.doctors.update((list) => list.map((d) => (d.id === updated.id ? updated : d)));
        }
      });
  }

  remove(doctor: Doctor): void {
    if (!confirm(`Delete doctor "${doctor.fullName}"?`)) {
      return;
    }

    this.doctorsService.delete(doctor.id).subscribe({
      next: () => {
        this.doctors.update((list) => list.filter((d) => d.id !== doctor.id));
        this.snackBar.open('Doctor deleted', 'OK', { duration: 3000 });
      },
      error: () => this.snackBar.open('Failed to delete the doctor', 'OK', { duration: 3000 }),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.specialisationsService.getAll().subscribe((items) => this.specialisations.set(items));
    this.doctorsService.getAll().subscribe({
      next: (items) => {
        this.doctors.set(items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
