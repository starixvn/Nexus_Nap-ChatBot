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

export const getRealValuesFromExtra = function (result, collection) {
    for (let i = 0; i < collection.length; i++) {
        const item = collection[i];
        if (item.children && item.children.length > 0) {
            getRealValuesFromExtra(result, item.children);
        } else {
            const value = item.realValue || item.node.props.realValue;

            if (!result.some(r => r === value)) {
                result.push(value);
            }
        }
    }
};

export const getRealValuesFromAllCheckNodes = function (result, collection) {
    for (let i = 0; i < collection.length; i++) {
        const item = collection[i].node ? collection[i].node : collection[i];
        if (item.props && item.props.children && item.props.children.length > 0) {
            getRealValuesFromAllCheckNodes(result, item.props.children);
        } else {
            const value = item.realValue || item.props.realValue;

            if (!result.some(r => r === value)) {
                result.push(value);
            }
        }
    }
};

export const getRealValuesFromCollection = function (result, collection) {
    if (collection.length > 0) {
        if (collection[0].props) {
            getRealValuesFromAllCheckNodes(result, collection);
        } else {
            getRealValuesFromExtra(result, collection);
        }
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
}