import dotenv from 'dotenv'
dotenv.config();

export const API = process.env.NODE_ENV === 'production' ? process.env.REACT_APP_PROD_API_URL : process.env.REACT_APP_DEV_API_URL;

export const CAPTCHA_KEY = process.env.NODE_ENV === 'production' ? process.env.REACT_APP_PROD_CAPTCHA_KEY : process.env.REACT_APP_DEV_CAPTCHA_KEY;