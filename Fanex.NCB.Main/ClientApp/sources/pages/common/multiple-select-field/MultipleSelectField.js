import React, { Component } from 'react';
import { FormFeedback } from 'reactstrap';
import Labels from '../../common/Labels';
import MultipleSelect from '@9softstudio/react-multiselect';
import '@9softstudio/react-multiselect/dist/style.css';
import './MultipleSelectField.scss';

export class MultipleSelectField extends Component {
    constructor(props) {
        super(props);
    }

    onChange = value => {
        this.props.onChange({
            value: value,
            isValid: this.isValid(value)
        });
    }

    isValid = value => value !== '';

    render() {
        let defaultTexts = { SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' };
        return (
            <div style={{ width: this.props.width ? this.props.width : 'auto' }}
                className={this.props.isSummited && !this.isValid(this.props.selectedValue) ? 'invalid multiple-select-field' : 'multiple-select-field'}
            >
                {
                    this.props.data &&
                    <MultipleSelect id={this.props.id}
                        dataSource={this.props.data}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        hasSearchBox={this.props.isSearchable}
                        hasAllOption={this.props.data.length > 0}
                        texts={this.props.texts || defaultTexts}
                        isAllTextShown={this.props.isAllTextShown || false}
                        onChange={(currentItem, selectedItemsKey) => this.onChange(selectedItemsKey)}
                    />
                }
                {this.props.isSummited && !this.isValid(this.props.selectedValue) && <FormFeedback>This field is required.</FormFeedback>}
            </div>
        );
    }
}