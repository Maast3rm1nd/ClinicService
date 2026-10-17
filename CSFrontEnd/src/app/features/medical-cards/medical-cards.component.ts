import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MedicalCard } from '../../core/models/medical-card.model';
import { Diagnosis, diagnosisLabel } from '../../core/models/diagnosis.model';
import { Policy } from '../../core/models/policy.model';
import { Patient } from '../../core/models/patient.model';
import { MedicalCardsService } from '../../core/services/medical-cards.service';
import { PatientsService } from '../../core/services/patients.service';
import { PoliciesService } from '../../core/services/policies.service';
import { DiagnosesService } from '../../core/services/diagnoses.service';

@Component({
  selector: 'app-medical-cards',
  standalone: true,
  imports: [CommonModule, RouterLink, MatProgressSpinnerModule, MatTableModule],
  templateUrl: './medical-cards.component.html',
  styleUrl: './medical-cards.component.css',
})
export class MedicalCardsComponent {
  readonly loading = signal(true);
  readonly cards = signal<MedicalCard[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly policies = signal<Policy[]>([]);
  readonly diagnoses = signal<Diagnosis[]>([]);
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
    private readonly policiesService: PoliciesService,
    private readonly diagnosesService: DiagnosesService,
    private readonly snackBar: MatSnackBar,
  ) {
    this.load();
  }

  patientName(card: MedicalCard): string {
    const patient = this.patients().find((item) => item.entityId === card.patient);
    return patient?.shortName || patient?.fullName || card.patient;
  }

  policyName(card: MedicalCard): string {
    if (!card.policy) {
      return '—';
    }
    return this.policies().find((item) => item.entityId === card.policy)?.medicalPolicyNumber ?? '—';
  }

  diagnosesNames(card: MedicalCard): string {
    const byId = new Map(this.diagnoses().map((item) => [item.entityId, diagnosisLabel(item)]));
    const names = (card.diagnoses ?? []).map((id) => byId.get(id)).filter((name) => !!name);
    return names.length > 0 ? names.join(', ') : '—';
  }

  private load(): void {
    // Policies are available to administrators only, so a failure just leaves the column empty.
    this.policiesService.getAll().subscribe({
      next: (items) => this.policies.set(items),
      error: () => undefined,
    });
    this.diagnosesService.getAll().subscribe({
      next: (items) => this.diagnoses.set(items),
      error: () => undefined,
    });

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
