import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MovementService } from '../../services/movement.service';
import { Product, ProductCosif, AddManualMovementRequest } from '../../models';

@Component({
  selector: 'app-movement-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './movement-form.component.html',
  styleUrls: ['./movement-form.component.css']
})
export class MovementFormComponent implements OnInit {
  @Input() products: Product[] = [];
  @Input() productCosifs: ProductCosif[] = [];

  @Output() productSelected = new EventEmitter<string>();
  @Output() movementCreated = new EventEmitter<void>();
  @Output() formReset = new EventEmitter<void>();

  movementForm: FormGroup;
  isFormEnabled = false;
  submitting = false;
  errorMessage: string = '';
  errorList: string[] = [];

  constructor(
    private fb: FormBuilder,
    private movementService: MovementService
  ) {
    this.movementForm = this.fb.group({
      month: ['', [Validators.required, Validators.min(1), Validators.max(12)]],
      year: ['', [Validators.required, Validators.min(2020), Validators.max(2030)]],
      productCode: ['', Validators.required],
      cosifCode: ['', Validators.required],
      value: ['', [Validators.required, Validators.min(0.01)]],
      description: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  ngOnInit(): void {
    // Disable form initially
    this.movementForm.disable();
  }

  onNewClick(): void {
    this.isFormEnabled = true;
    this.movementForm.enable();
    setTimeout(() => {
      const monthInput = document.querySelector('input[formControlName="month"]') as HTMLInputElement;
      if (monthInput) {
        monthInput.focus();
      }
    }, 100);
  }

  onClearClick(): void {
    this.movementForm.reset();
    this.isFormEnabled = false;
    this.movementForm.disable();
    this.formReset.emit();
  }

  onProductChange(): void {
    const productCode = this.movementForm.get('productCode')?.value;
    this.productSelected.emit(productCode);
    this.movementForm.get('cosifCode')?.setValue('');
  }

  onSubmit(): void {
    if (this.movementForm.valid && !this.submitting) {
      this.submitting = true;
      this.errorMessage = '';
      this.errorList = [];

      const formValue = this.movementForm.value;
      // Gera data local no formato 'YYYY-MM-DDTHH:mm:ss'
      const now = new Date();
      const localDate = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
        .toISOString()
        .slice(0, 19); // 'YYYY-MM-DDTHH:mm:ss'
      const movement: AddManualMovementRequest = {
        month: formValue.month,
        year: formValue.year,
        productCode: formValue.productCode,
        cosifCode: formValue.cosifCode,
        description: formValue.description,
        movementDate: localDate, // agora no horário local
        userCode: 'USER001', // TODO: Get from auth service
        value: formValue.value
      };

      this.movementService.createMovement(movement).subscribe({
        next: (response) => {
          if (response.success) {
            this.movementCreated.emit();
            this.onClearClick();
            alert('Movimento criado com sucesso!');
          } else {
            this.errorMessage = response.message || 'Erro desconhecido';
            this.errorList = response.errors || [];
          }
          this.submitting = false;
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Erro ao criar movimento. Tente novamente.';
          this.errorList = error.error?.errors || [];
          this.submitting = false;
        }
      });
    }
  }

  getMonthErrorMessage(): string {
    const control = this.movementForm.get('month');
    if (control?.errors) {
      if (control.errors['required']) return 'Mês é obrigatório';
      if (control.errors['min'] || control.errors['max']) return 'Mês deve estar entre 1 e 12';
    }
    return '';
  }

  getYearErrorMessage(): string {
    const control = this.movementForm.get('year');
    if (control?.errors) {
      if (control.errors['required']) return 'Ano é obrigatório';
      if (control.errors['min'] || control.errors['max']) return 'Ano deve estar entre 2020 e 2030';
    }
    return '';
  }

  getValueErrorMessage(): string {
    const control = this.movementForm.get('value');
    if (control?.errors) {
      if (control.errors['required']) return 'Valor é obrigatório';
      if (control.errors['min']) return 'Valor deve ser maior que zero';
    }
    return '';
  }

  getDescriptionErrorMessage(): string {
    const control = this.movementForm.get('description');
    if (control?.errors) {
      if (control.errors['required']) return 'Descrição é obrigatória';
      if (control.errors['minlength']) return 'Descrição deve ter pelo menos 3 caracteres';
    }
    return '';
  }
}
