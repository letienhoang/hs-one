export interface INavAttributes {
    policyName?: string;
    [key: string]: any; 
  }
  
  export interface INavChild {
    name: string;
    url: string;
    icon?: string;
    iconComponent?: { name: string };
    attributes?: INavAttributes;
  }
  
  export interface INavData {
    name?: string;
    url?: string;
    title?: boolean;
    iconComponent?: { name: string };
    icon?: string;
    children?: INavChild[];
    attributes?: INavAttributes;
  }
  