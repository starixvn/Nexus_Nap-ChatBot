import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Option from './Option';
import Translation from './translation';
import { DEFAULT_CHECKED_ALL } from '../constants';

export default class OptionAll extends Component {
    static propTypes = {
        language: PropTypes.string,
        onChange: PropTypes.func,
        id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]).isRequired,
        checked: PropTypes.bool
    };

    static defaultProps = {
        language: 'en-US',
        checked: DEFAULT_CHECKED_ALL
    };

    constructor(props) {
        super(props);

        this.itemData = {
            key: 'All',
            value: Translation[this.props.language].All
        };
    }

    changeHandler = ({ checked }) => {
        this.props.onChange && this.props.onChange(checked);
    };

    render() {
        const { id, checked } = this.props;
        this.itemData.checked = checked;

        return (
            <ul className="multiple-select-options multiple-select-option-all">
                <Option
                    id={`${id}-optionItemAll`}
                    itemData={this.itemData}
                    onChange={this.changeHandler} />
            </ul>
        );
    }
}