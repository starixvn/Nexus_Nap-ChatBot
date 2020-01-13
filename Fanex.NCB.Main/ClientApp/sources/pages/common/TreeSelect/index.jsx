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
            const value = item.realValue || (item.node ? item.node.props.realValue : item.props.realValue);

            if (!result.some(r => r === value)) {
                result.push(value);
            }
        }
    }
};