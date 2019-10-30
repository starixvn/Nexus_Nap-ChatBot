import Site from '../lib/site';

const errorResponseHandler = (error) => {
    if (error.response) {
        switch (error.response.status) {
            case 401:
                window.location = Site.resolveClientUrl('Error/UnauthorizedUser');
                break;

            case 404:
                window.location = Site.resolveClientUrl('Error/PageNotFound');
                break;

            case 500:
                window.location = Site.resolveClientUrl('Error/InternalServerError');
                break;
        }
    }
};

export default errorResponseHandler;