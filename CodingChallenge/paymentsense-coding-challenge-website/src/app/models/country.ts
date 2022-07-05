import { Currency } from './currency';
import { Language } from './language';

export interface ICountry {
  alpha3Code: string;
  name: string;
  flag: string;
  population: number;
  timezones: string[];
  currencies: Currency[];
  languages: Language[];
  capital: string;
  borders: string[];
  cachedFlag: string;
}
