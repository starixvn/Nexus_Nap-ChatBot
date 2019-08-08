import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export const setSettings = data => {
    return {
        type: types.SETTING_DATA,
        data: data
    };
};

export const getSettings = () => {
    const url = Site.resolveClientUrl('NewNotification/GetSettings');
    const data = {
        notificationTypes: [{ id: 1, name: 'a' }, { id: 2, name: 'b' }],
        products: [{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }]
    };

    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setSettings(data)); //test
        });
    };
};

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
export const setTemplateTypes = data => {
    return {
        type: types.TEMPLATE_TYPE_DATA,
        data: data
    };
};

export const getTemplateTypes = () => {
    const url = Site.resolveClientUrl('NewNotification/GetNotificationTemplate');
    const data = [{ id: 1, name: 'template 1' }, { id: 2, name: 'template 2' }];

    return (dispatch) => {
        return axios.get(url).then(response => {
            dispatch(setTemplateTypes(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setTemplateTypes(data));
        }) //test
    }
};

// Template
export const setTemplate = data => {
    return {
        type: types.TEMPLATE_DATA,
        data: data
    };
};

export const getTemplate = () => {
    const url = Site.resolveClientUrl('NewNotification/GetTemplate');
    const data = {
        title: 'title hh {UM date}',
        content: 'good day! \n {UM time} something \n {UM time}'
            + 'and {End time} <br/> {UM time} and {End time} <br/> {UM time}'
            + 'and {End time} <br/> {a very long parameter here} {ha ha} cc'
    };

    return dispatch => {
        axios.get(url).then(response => {
            dispatch(setTemplate(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setTemplate(data));
        });
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

const actions = {
    setSettings,
    getSettings,
    setNotificationTypes,
    getNotificationTypes,
    setTemplateTypes,
    getTemplateTypes,
    setTemplate,
    getTemplate,
    setProducts,
    getProducts,
    setRecipients,
    getRecipients
};

export default actions;