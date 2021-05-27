import React, { PureComponent, Fragment } from 'react';

export default class TreeNode extends PureComponent {
    handleChange = event => {
        const itemkey = this.props.data.key;
        this.props.onSelect(itemkey, event.target.checked);
    }

    render() {
        const { id, data, floatData } = this.props;
        const itemIndex = floatData.findIndex(item => item.key === data.key);

        if (itemIndex < 0) {
            return null;
        }

        return (
            <Fragment>
                <div className="checkbox-wrapper">
                    <input type="checkbox" id={`${id}-${data.key}`} checked={floatData[itemIndex].checked} onChange={this.handleChange} />
                    <label htmlFor={`${id}-${data.key}`}> {data.text}</label>
                </div>
                {
                    data.items && data.items.length > 0 &&
                    <ul>
                        {
                            data.items.map(item =>
                                <li key={item.key} className="multiselect-treeview-item is-child">
                                    <TreeNode
                                        data={item}
                                        floatData={floatData}
                                        onSelect={this.props.onSelect}
                                    />
                                </li>
                            )
                        }
                    </ul>
                }
            </Fragment>
        );
    }
}