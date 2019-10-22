import Site from '../lib/site';

const errorResponseHandler = (error) => {
    if (error.response) {
        let url;

        switch (error.response.status) {
            case 401:
                url = Site.resolveClientUrl('Error/UnauthorizedUser');
                break;

            case 404:
                url = Site.resolveClientUrl('Error/PageNotFound');
                break;

            case 500:
                url = Site.resolveClientUrl('Error/InternalServerError');
                break;
            default:
                url = Site.resolveClientUrl('Auth/Index');
        }

        window.location = url;
    }
};

export default errorResponseHandler;