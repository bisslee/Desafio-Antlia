import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManualMovement } from '../../models';

@Component({
  selector: 'app-movement-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movement-grid.component.html',
  styleUrls: ['./movement-grid.component.css']
})
export class MovementGridComponent {
  @Input() movements: ManualMovement[] = [];
  @Input() page = 1;
  @Input() totalPages = 1;
  @Input() total = 0;
  @Output() pageChange = new EventEmitter<number>();

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.pageChange.emit(page);
    }
  }

  prevPage() {
    this.goToPage(this.page - 1);
  }

  nextPage() {
    this.goToPage(this.page + 1);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatMonthYear(month: number, year: number): string {
    return `${month.toString().padStart(2, '0')}/${year}`;
  }

  getProductDescription(movement: ManualMovement): string {
    return movement.product?.description || movement.productCode || 'N/A';
  }

  getCosifDescription(movement: ManualMovement): string {
    return movement.productCosif?.cosifCode || movement.cosifCode || 'N/A';
  }
}
