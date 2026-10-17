import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { CardNotesService } from '../../../core/services/card-notes.service';
import { DiagnosesService } from '../../../core/services/diagnoses.service';
import { MedicalCardsService } from '../../../core/services/medical-cards.service';
import { PatientsService } from '../../../core/services/patients.service';
import { CardNote } from '../../../core/models/card-note.model';
import { Diagnosis, diagnosisLabel } from '../../../core/models/diagnosis.model';
import { MedicalCard } from '../../../core/models/medical-card.model';
import { Patient } from '../../../core/models/patient.model';

@Component({
  selector: 'app-medical-card-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './medical-card-detail.component.html',
  styleUrl: './medical-card-detail.component.css',
})
export class MedicalCardDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly cardsService = inject(MedicalCardsService);
  private readonly notesService = inject(CardNotesService);
  private readonly diagnosesService = inject(DiagnosesService);
  private readonly patientsService = inject(PatientsService);
  private readonly snackBar = inject(MatSnackBar);
  protected readonly auth = inject(AuthService);

  private readonly cardEntityId = this.route.snapshot.paramMap.get('entityId') ?? '';
  private readonly appointmentId = this.route.snapshot.queryParamMap.get('appointment');

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly adding = signal(false);
  readonly card = signal<MedicalCard | null>(null);
  readonly patient = signal<Patient | null>(null);
  readonly notes = signal<CardNote[]>([]);
  readonly diagnoses = signal<Diagnosis[]>([]);

  readonly sortedNotes = computed(() =>
    [...this.notes()].sort((a, b) => b.dateNote.localeCompare(a.dateNote)),
  );

  readonly form = this.fb.nonNullable.group({
    dateNote: [this.toLocalInputValue(new Date()), Validators.required],
    patientsComplaints: [''],
    diagnoses: this.fb.nonNullable.control<string[]>([]),
    newDiagnoses: this.fb.array([this.createDiagnosisGroup()]),
  });

  constructor() {
    this.load();
  }

  readonly diagnosisLabel = diagnosisLabel;

  get newDiagnoses(): FormArray {
    return this.form.controls.newDiagnoses;
  }

  addDiagnosisRow(): void {
    this.newDiagnoses.push(this.createDiagnosisGroup());
  }

  removeDiagnosisRow(index: number): void {
    this.newDiagnoses.removeAt(index);
  }

  diagnosisText(entityId: string): string {
    const diagnosis = this.diagnoses().find((d) => d.entityId === entityId);
    return diagnosis ? diagnosisLabel(diagnosis) : '—';
  }

  private createDiagnosisGroup() {
    return this.fb.nonNullable.group({ icdCode: [''], name: [''] });
  }

  submit(): void {
    const card = this.card();
    const doctor = this.auth.currentUser()?.id;
    if (!card || !doctor) {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { dateNote, patientsComplaints, diagnoses, newDiagnoses } = this.form.getRawValue();
    const created = (newDiagnoses as { icdCode: string; name: string }[])
      .map((item) => ({ icdCode: item.icdCode.trim(), name: item.name.trim() || null }))
      .filter((item) => item.icdCode.length > 0);

    this.saving.set(true);
    this.notesService
      .create({
        medicalCard: card.entityId,
        doctor,
        appointment: this.appointmentId,
        dateNote: new Date(dateNote).toISOString(),
        patientsComplaints: patientsComplaints.trim() || null,
        diagnoses,
        newDiagnoses: created,
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.adding.set(false);
          this.form.reset({ dateNote: this.toLocalInputValue(new Date()) });
          this.newDiagnoses.clear();
          this.addDiagnosisRow();
          this.snackBar.open('Note added', 'OK', { duration: 3000 });
          this.load();
        },
        error: (error: HttpErrorResponse) => {
          this.saving.set(false);
          this.snackBar.open(error.error?.message ?? 'Failed to add the note', 'OK', {
            duration: 5000,
          });
        },
      });
  }

  private load(): void {
    this.loading.set(true);
    this.cardsService.getByEntityId(this.cardEntityId).subscribe({
      next: (card) => {
        this.card.set(card);
        if (!card) {
          this.loading.set(false);
          return;
        }
        forkJoin({
          notes: this.notesService.getByMedicalCard(card.entityId),
          diagnoses: this.diagnosesService.getByMedicalCard(card.entityId),
          patients: this.patientsService.getAll(),
        }).subscribe({
          next: ({ notes, diagnoses, patients }) => {
            this.notes.set(notes);
            this.diagnoses.set(diagnoses);
            this.patient.set(patients.find((p) => p.entityId === card.patient) ?? null);
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
            this.snackBar.open('Could not load the medical card', 'Dismiss', { duration: 4000 });
          },
        });
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load the medical card', 'Dismiss', { duration: 4000 });
      },
    });
  }

  /** Formats a date for a datetime-local input (local time, minute precision). */
  private toLocalInputValue(date: Date): string {
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}
