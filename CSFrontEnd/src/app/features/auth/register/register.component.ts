import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EmployeeWorkStatus, employeeWorkStatusLabels, PersonType } from '../../../core/models/enums';
import { PersonsService } from '../../../core/services/persons.service';
import { DoctorsService } from '../../../core/services/doctors.service';
import { SpecialisationsService } from '../../../core/services/specialisations.service';
import { Specialisation } from '../../../core/models/specialisation.model';

type AccountKind = 'person' | 'doctor';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatButtonToggleModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly personsService = inject(PersonsService);
  private readonly doctorsService = inject(DoctorsService);
  private readonly specialisationsService = inject(SpecialisationsService);
  private readonly snackBar = inject(MatSnackBar);

  readonly kind = signal<AccountKind>('person');
  readonly saving = signal(false);
  readonly createdLogin = signal<string | null>(null);
  readonly specialisations = signal<Specialisation[]>([]);
  readonly workStatuses = Object.values(EmployeeWorkStatus);
  readonly workStatusLabels = employeeWorkStatusLabels;

  readonly personForm = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    shortName: [''],
    login: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(12)]],
  });

  readonly doctorForm = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    shortName: [''],
    login: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(12)]],
    specialisations: this.fb.nonNullable.control<string[]>([], Validators.required),
    employeeWorkStatus: [EmployeeWorkStatus.AtWork, Validators.required],
  });

  readonly activeForm = computed(() => (this.kind() === 'person' ? this.personForm : this.doctorForm));

  constructor() {
    this.specialisationsService.getAll().subscribe((items) => this.specialisations.set(items));
  }

  setKind(kind: AccountKind): void {
    this.kind.set(kind);
  }

  submit(): void {
    if (this.kind() === 'person') {
      this.submitPerson();
    } else {
      this.submitDoctor();
    }
  }

  private submitPerson(): void {
    if (this.personForm.invalid) {
      this.personForm.markAllAsTouched();
      return;
    }

    const { fullName, shortName, login, password } = this.personForm.getRawValue();
    this.saving.set(true);
    this.personsService
      .create({ fullName, shortName: shortName || null, login, password, type: PersonType.Administrator })
      .subscribe({
        next: () => this.onSuccess(login),
        error: (error: HttpErrorResponse) => this.onError(error),
      });
  }

  private submitDoctor(): void {
    if (this.doctorForm.invalid) {
      this.doctorForm.markAllAsTouched();
      return;
    }

    const { fullName, shortName, login, password, specialisations, employeeWorkStatus } =
      this.doctorForm.getRawValue();
    this.saving.set(true);
    this.doctorsService
      .create({
        id: crypto.randomUUID(),
        fullName,
        shortName: shortName || null,
        login,
        password,
        specialisations,
        employeeWorkStatus,
      })
      .subscribe({
        next: () => this.onSuccess(login),
        error: (error: HttpErrorResponse) => this.onError(error),
      });
  }

  private onSuccess(login: string): void {
    this.saving.set(false);
    this.createdLogin.set(login);
  }

  private onError(error: HttpErrorResponse): void {
    this.saving.set(false);
    const message = error.error?.message ?? 'Failed to create account';
    this.snackBar.open(message, 'OK', { duration: 4000 });
  }
}
