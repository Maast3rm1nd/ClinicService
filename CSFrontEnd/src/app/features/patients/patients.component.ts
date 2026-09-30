import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Patient } from '../../core/models/patient.model';
import { PatientsService } from '../../core/services/patients.service';

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatProgressSpinnerModule, MatTableModule],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.css',
})
export class PatientsComponent {
  readonly loading = signal(true);
  readonly patients = signal<Patient[]>([]);
  readonly displayedColumns = ['fullName', 'birthDate', 'phoneNumber', 'bloodGroup', 'allergies'];

  constructor(
    private readonly patientsService: PatientsService,
    private readonly snackBar: MatSnackBar,
  ) {
    this.patientsService.getAll().subscribe({
      next: (items) => {
        this.patients.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load patients.', 'Dismiss', { duration: 4000 });
      },
    });
  }
}
