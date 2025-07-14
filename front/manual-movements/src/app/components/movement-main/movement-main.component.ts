import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MovementService } from '../../services/movement.service';
import { ProductService } from '../../services/product.service';
import { MovementFormComponent } from '../movement-form/movement-form.component';
import { MovementGridComponent } from '../movement-grid/movement-grid.component';
import { ManualMovement, Product, ProductCosif, GetManualMovementResponse } from '../../models';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-movement-main',
  standalone: true,
  imports: [CommonModule, MovementFormComponent, MovementGridComponent],
  templateUrl: './movement-main.component.html',
  styleUrls: ['./movement-main.component.css']
})
export class MovementMainComponent implements OnInit {
  movements: ManualMovement[] = [];
  products: Product[] = [];
  productCosifs: ProductCosif[] = [];
  loading = false;
  error: string | null = null;

  // Paginação
  page = 1;
  offset = 5; // sempre 5 itens por página
  totalPages = 1;
  total = 0;

  constructor(
    private movementService: MovementService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    console.log('🔧 Environment config:', environment);
    console.log('🔧 API URL:', environment.apiUrl);
    console.log('🔧 Use Mock Data:', environment.useMockData);

    this.loadMovements();
    this.loadProducts();
  }

  loadMovements(page: number = 1): void {
    this.loading = true;
    this.error = null;
    this.movementService.getMovements(
      undefined, undefined, undefined, undefined, undefined, // até active
      undefined, // active
      5, // Offset sempre 5
      page // Page atual
    ).subscribe({
      next: (response: GetManualMovementResponse) => {
        this.movements = response.data || [];
        this.page = page;
        this.total = response.total;
        this.totalPages = Math.ceil(this.total / 5) || 1;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Erro ao carregar movimentos';
        this.loading = false;
      }
    });
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.loadMovements(page);
    }
  }

  loadProducts(): void {
    console.log('📡 Carregando produtos...');
    this.productService.getProducts(undefined, undefined, true).subscribe({
      next: (response) => {
        console.log('✅ Produtos carregados:', response);
        this.products = response.data || [];
        console.log('📊 Total de produtos:', this.products.length);
      },
      error: (error) => {
        console.error('❌ Erro ao carregar produtos:', error);
      }
    });
  }

  onProductSelected(productCode: string): void {
    console.log('📡 Produto selecionado:', productCode);
    if (productCode) {
      this.productService.getProductCosifs(productCode, undefined, undefined, true).subscribe({
        next: (response) => {
          console.log('✅ COSIFs carregados:', response);
          this.productCosifs = response.data || [];
          console.log('📊 Total de COSIFs:', this.productCosifs.length);
        },
        error: (error) => {
          console.error('❌ Erro ao carregar COSIFs:', error);
        }
      });
    } else {
      this.productCosifs = [];
    }
  }

  onMovementCreated(): void {
    console.log('🔄 Recarregando movimentos após criação...');
    this.loadMovements();
  }

  onFormReset(): void {
    this.productCosifs = [];
  }
}
