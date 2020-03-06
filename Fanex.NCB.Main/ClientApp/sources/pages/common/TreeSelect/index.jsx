import React from 'react';
import TreeSelect, { SHOW_PARENT } from 'rc-tree-select';
import 'rc-tree-select/assets/index.css';
import './TreeSelect.scss';

export default React.memo(function (props) {
    const { className, ...rest } = props;
    return (
        <TreeSelect
            className={`form-control ${className}`}
            treeCheckable
            labelInValue
            showCheckedStrategy={SHOW_PARENT}
            treeNodeFilterProp="title"
            {...rest}
        />
    );
});

const pushAllRealValueToResult = (result, collection) => {
    for (let i = 0; i < collection.length; i++) {
        const element = collection[i];
        if (element.children && element.children.length > 0) {
            pushAllRealValueToResult(result, element.children);
        } else {
            if (!result.some(r => r === element.realValue)) {
                result.push(element.realValue);
            }
        }
    }
};

const pushRealValueFromCollection = (result, element) => {
    if (element.children && element.children.length > 0) {
        pushAllRealValueToResult(result, element.children);
    } else {
        if (!result.some(r => r === element.realValue)) {
            result.push(element.realValue);
        }
    }
};

const getRealValueFromNode = (result, recipient, collection) => {
    for (let i = 0; i < collection.length; i++) {
        const element = collection[i];
        if (recipient.value === element.value) {
            pushRealValueFromCollection(result, element);
        } else {
            if (element.children && element.children.length > 0) {
                getRealValueFromNode(result, recipient, element.children);
            }
        }
    }
};

export const getRealValueFromTreeSelection = function (result, values, collection) {
    for (let i = 0; i < values.length; i++) {
        getRealValueFromNode(result, values[i], collection);
    }
};

export const getLeafTitleFromCollection = function (result, collection) {
    for (let i = 0; i < collection.length; i++) {
        const item = collection[i];
        if (item.children && item.children.length > 0) {
            getLeafTitleFromCollection(result, item.children);
        } else {
            const title = item.title || item.node.props.title;

            if (!result.some(r => r === title)) {
                result.push(title);
            }
        }
    }
};