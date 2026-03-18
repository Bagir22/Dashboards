export interface MetabaseDatasetQuery {
  'lib/type': string;
  database: number;
  stages: Array<{
      'lib/type': string;
      'source-table': number | string;
      [key: string]: any;
  }>;
  [key: string]: any;
}

export interface MetabaseCard {
  id: number;
  name: string;
  description: string | null;
  display: string;
  visualization_settings: Record<string, any>;
  parameters: any[];
  entity_id: string;
  dataset_query: MetabaseDatasetQuery | Record<string, any>;
  [key: string]: any;
}

export interface MetabaseDashcard {
  size_x: number;
  dashboard_tab_id: number;
  series: any[];
  inline_parameters: any[];
  card: MetabaseCard;
  col: number;
  id: number;
  parameter_mappings: any[];
  card_id: number;
  visualization_settings: Record<string, any>;
  size_y: number;
  dashboard_id: number;
  row: number;
  [key: string]: any;
}

export interface MetabaseTab {
  id: number;
  dashboard_id: number;
  name: string;
  position: number;
  entity_id: string;
  created_at: string;
  updated_at: string;
  [key: string]: any;
}

export interface MetabaseDashboard {
  description: string | null;
  dashcards: MetabaseDashcard[];
  tabs: MetabaseTab[];
  name: string;
  width: string;
  id: number;
  param_fields: Record<string, any>;
  parameters: any[];
  auto_apply_filters: boolean;
  [key: string]: any;
}

export interface TabCard {
  id: number;
  name: string;
  display: string;
  description?: string | null;
}

export interface TabInfo {
  id: number | null;
  name?: string;
  cards: TabCard[];
}

export interface TabMap {
  [tabId: number]: TabInfo;
}

export interface Dashboard {
  name: string;
  id: number;
  public_uuid: string;
}

export interface MetabaseAuth {
  username?: string;
  password?: string;
}
