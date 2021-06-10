import { KEY_NAME, VALUE_NAME, STATUS_NAME } from './constants';

export const convertDataSourceToState = ({ keyField, valueField, statusField, dataSource }) => {
    return dataSource.map(item => {
        return {
            [KEY_NAME]: item[keyField],
            [VALUE_NAME]: item[valueField],
            [STATUS_NAME]: item[statusField] ? item[statusField] : false,
            visible: true
        };
    });
};

const functions = {
    convertDataSourceToState
};

export default functions;