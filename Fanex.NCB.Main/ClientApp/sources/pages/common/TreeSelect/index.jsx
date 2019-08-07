import React from 'react';
import data from "./data.json";
import TreeSelect, { SHOW_PARENT } from 'rc-tree-select';
import "rc-tree-select/assets/index.css";
import "./TreeSelect.scss";

export default React.memo(function (props) {
    return (
        <TreeSelect
            className={`form-control ${props.invalid && 'is-invalid'}`}
            treeData={data}
            treeCheckable
            labelInValue
            showCheckedStrategy={SHOW_PARENT}
            treeNodeFilterProp="label"
            {...props}
        />
    );
});