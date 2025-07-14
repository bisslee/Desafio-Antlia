export enum DataStatus {
  Created = 'created',
  Active = 'active',
  Inactive = 'inactive',
  Deleted = 'deleted'
}

// Product Models
export interface Product {
  id: string;
  createdAt: string;
  createdBy: string | null;
  updatedAt: string;
  updatedBy: string | null;
  status: DataStatus;
  productCode: string | null;
  description: string | null;
  productCosifs?: ProductCosif[];
  manualMovements?: ManualMovement[];
}

export interface ProductDto {
  id: string;
  productCode: string | null;
  description: string | null;
}

export interface GetProductResponse {
  success: boolean;
  data: Product[] | null;
  isSuccess: boolean;
  statusCode: number;
  message: string | null;
  exception: any;
  errors: string[] | null;
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ProductCosif Models
export interface ProductCosif {
  id: string;
  createdAt: string;
  createdBy: string | null;
  updatedAt: string;
  updatedBy: string | null;
  status: DataStatus;
  productCode: string | null;
  cosifCode: string | null;
  classificationCode: string | null;
  product?: Product;
  manualMovements?: ManualMovement[];
}

export interface ProductCosifDto {
  id: string;
  productCode: string | null;
  cosifCode: string | null;
  classificationCode: string | null;
}

export interface GetProductCosifResponse {
  success: boolean;
  data: ProductCosif[] | null;
  isSuccess: boolean;
  statusCode: number;
  message: string | null;
  exception: any;
  errors: string[] | null;
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ManualMovement Models
export interface ManualMovement {
  id: string;
  createdAt: string;
  createdBy: string | null;
  updatedAt: string;
  updatedBy: string | null;
  status: DataStatus;
  month: number;
  year: number;
  launchNumber: number;
  productCode: string | null;
  cosifCode: string | null;
  description: string | null;
  movementDate: string;
  userCode: string | null;
  value: number;
  product?: Product;
  productCosif?: ProductCosif;
}

export interface ManualMovementDto {
  id: string;
  month: number;
  year: number;
  launchNumber: number;
  productCode: string | null;
  cosifCode: string | null;
  description: string | null;
  movementDate: string;
  userCode: string | null;
  value: number;
}

export interface AddManualMovementRequest {
  month: number;
  year: number;
  productCode: string | null;
  cosifCode: string | null;
  description: string | null;
  movementDate: string;
  userCode: string | null;
  value: number;
}

export interface GetManualMovementResponse {
  success: boolean;
  data: ManualMovement[] | null;
  isSuccess: boolean;
  statusCode: number;
  message: string | null;
  exception: any;
  errors: string[] | null;
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface AddManualMovementResponse {
  success: boolean;
  data: ManualMovementDto;
  isSuccess: boolean;
  statusCode: number;
  message: string | null;
  exception: any;
  errors: string[] | null;
}
