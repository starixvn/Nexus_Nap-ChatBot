import axios from 'axios';
import Site from './site';
import errorResponseHandler from './errorResponseHandler';

export default class DataService {
    static async getDataAsync(url, params) {
        return await axios.get(
            url,
            {
                params: params
            }).then(response => response).catch(errorResponseHandler);
    }

    static async postDataAsync(url, params) {
        return await axios.post(
            url,
            params,
            {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': Site.getXsrfToken()
                }
            }).then(response => response).catch(errorResponseHandler);
    }
}