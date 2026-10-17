import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSlideToggleChange, MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DoctorsService } from '../../core/services/doctors.service';
import { SpecialisationsService } from '../../core/services/specialisations.service';
import { Doctor } from '../../core/models/doctor.model';
import { Specialisation } from '../../core/models/specialisation.model';
import { employeeWorkStatusLabels } from '../../core/models/enums';
import { DoctorDialogComponent } from './doctor-dialog/doctor-dialog.component';
import { TwoFactorSecretDialogComponent } from './two-factor-secret-dialog/two-factor-secret-dialog.component';
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
    MatSlideToggleModule,
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
  readonly displayedColumns = ['fullName', 'login', 'specialisations', 'status', 'twoFactor', 'actions'];

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

  toggleTwoFactor(doctor: Doctor, change: MatSlideToggleChange): void {
    const enabled = change.checked;
    this.doctorsService.setTwoFactor(doctor.id, enabled).subscribe({
      next: (result) => {
        this.doctors.update((list) =>
          list.map((d) => (d.id === doctor.id ? { ...d, twoFactorEnabled: result.enabled } : d)),
        );
        if (result.secret) {
          this.dialog.open(TwoFactorSecretDialogComponent, {
            width: '440px',
            data: { doctorName: doctor.fullName, login: doctor.login, secret: result.secret },
          });
        } else {
          this.snackBar.open('Two-factor authentication disabled', 'OK', { duration: 3000 });
        }
      },
      error: () => {
        change.source.checked = !enabled;
        this.snackBar.open('Failed to change two-factor authentication', 'OK', { duration: 3000 });
      },
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
