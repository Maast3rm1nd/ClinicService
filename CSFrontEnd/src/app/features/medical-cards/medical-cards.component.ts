import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MedicalCard } from '../../core/models/medical-card.model';
import { Patient } from '../../core/models/patient.model';
import { MedicalCardsService } from '../../core/services/medical-cards.service';
import { PatientsService } from '../../core/services/patients.service';

@Component({
  selector: 'app-medical-cards',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatTableModule],
  templateUrl: './medical-cards.component.html',
  styleUrl: './medical-cards.component.css',
})
export class MedicalCardsComponent {
  readonly loading = signal(true);
  readonly cards = signal<MedicalCard[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly displayedColumns = [
    'recordNumber',
    'patient',
    'policy',
    'diagnoses',
    'creationDateTime',
  ];

  constructor(
    private readonly cardsService: MedicalCardsService,
    private readonly patientsService: PatientsService,
    private readonly snackBar: MatSnackBar,
  ) {
    this.load();
  }

  patientName(card: MedicalCard): string {
    const patient = this.patients().find((item) => item.entityId === card.patient);
    return patient?.shortName || patient?.fullName || card.patient;
  }

  private load(): void {
    this.loading.set(true);
    let cardsLoaded = false;
    let patientsLoaded = false;
    const finish = () => {
      if (cardsLoaded && patientsLoaded) {
        this.loading.set(false);
      }
    };
    this.cardsService.getAll().subscribe({
      next: (items) => {
        this.cards.set(items);
        cardsLoaded = true;
        finish();
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load medical cards.', 'Dismiss', { duration: 4000 });
      },
    });
    this.patientsService.getAll().subscribe({
      next: (items) => {
        this.patients.set(items);
        patientsLoaded = true;
        finish();
      },
      error: () => {
        patientsLoaded = true;
        finish();
      },
    });
  }
}
