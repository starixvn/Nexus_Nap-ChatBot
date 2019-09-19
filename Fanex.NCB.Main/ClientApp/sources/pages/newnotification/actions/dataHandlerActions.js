import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';
// Notification Type
export const setNotificationTypes = data => {
    return {
        type: types.NOTIFICATION_TYPE_DATA,
        data: data
    };
};

export const getNotificationTypes = () => {
    const url = Site.resolveClientUrl('Common/GetNotificationTypes');
    const data = [{ id: 1, name: 'a' }, { id: 2, name: 'b' }];

    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setNotificationTypes(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setNotificationTypes(data));
        }); //test
    };
};

// Notification Template
export const setTemplates = data => {
    return {
        type: types.TEMPLATE_DATA,
        data: data
    };
};

export const getTemplates = (notificationTypeId) => {
    const url = Site.resolveClientUrl('NewNotification/GetTemplates');

    return (dispatch) => {
        return axios.get(url, { params: { notificationTypeId } }).then(response => {
            dispatch(setTemplates(response.data));
        }).catch(errorResponseHandler);
    };
};

// Product
export const setProducts = data => {
    return {
        type: types.PRODUCT_DATA,
        data: data
    };
};

export const getProducts = () => {
    const url = Site.resolveClientUrl('Common/GetProducts');
    return dispatch => {
        axios.get(url).then(response => {
            dispatch(setProducts(response.data));
        }).catch(() => {
            errorResponseHandler();
        });
    };
};

// Recipient
export const setRecipients = data => {
    return {
        type: types.RECIPIENT_DATA,
        data: data
    };
};

export const getRecipients = (notificationTypeId, productId) => {
    const url = Site.resolveClientUrl('NewNotification/GetRecipients');
    return dispatch => {
        axios.get(url, {
            params: { notificationTypeId, productId }
        }).then(response => {
            dispatch(setRecipients(response.data));
        }).catch(() => {
            errorResponseHandler();
        });
    };
};

export const setCreateNewNotificationResult = data => {
    return {
        type: types.CREATENOTIFICATIONRESULT_DATA,
        data: data
    };
};

export const createNewNotification = (data) => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('NewNotification/Create'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setCreateNewNotificationResult(response.data));
    }).catch(errorResponseHandler);
};

export const toggleConfirmationPopup = (isOpen, content) => {
    const popupData = {
        isOpen,
        content
    };

    return {
        type: types.CONFIRMATIONPOPUP_DATA,
        data: popupData
    };
};

export const toggleResetConfirmationPopup = (isOpen, content) => {
    const popupData = {
        isOpen,
        content
    };

    return {
        type: types.RESET_CONFIRMATIONPOPUP_DATA,
        data: popupData
    };
};

export const toggleEditConfirmationPopup = (isOpen, content) => {
    const popupData = {
        isOpen,
        content
    };

    return {
        type: types.EDIT_CONFIRMATIONPOPUP_DATA,
        data: popupData
    };
};

const actions = {
    setNotificationTypes,
    getNotificationTypes,
    setTemplates,
    getTemplates,
    setProducts,
    getProducts,
    setRecipients,
    getRecipients,
    createNewNotification,
    setCreateNewNotificationResult,
    toggleConfirmationPopup,
    toggleResetConfirmationPopup,
    toggleEditConfirmationPopup
};

export default actions;