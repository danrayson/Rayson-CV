import { readFileSync } from 'fs';
import { join } from 'path';

const getConfig = () => {
  const env = process.env.NODE_ENV || 'development';
  const configPath = join(__dirname, `../config.${env}.json`);
  return JSON.parse(readFileSync(configPath, 'utf8'));
};
export default getConfig();