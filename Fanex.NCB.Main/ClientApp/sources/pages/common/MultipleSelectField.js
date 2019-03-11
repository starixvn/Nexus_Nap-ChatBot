import React, { Component } from 'react';
import { FormFeedback } from 'reactstrap';
import MultipleSelect from '@9softstudio/react-multiselect';
import './MultipleSelectField.scss';
import '@9softstudio/react-multiselect/dist/style.css';

export class MultipleSelectField extends Component {
    constructor(props) {
        super(props);

        this.state = {
            value: this.props.initialValue
        }
    }

    onChange = (value) => {
        this.setState(
            {
                value: value
            },
            () => {
                this.props.onChange({
                    value: this.state.value,
                    isValid: this.isValid()
                })
            }
        );
    }

    isValid = () => {
        return this.state.value != "";
    }   

    render() {
        return (
            <div style={{ width: this.props.width ? this.props.width : 'auto' }}
                className={this.props.isSummited && !this.isValid() ? "invalid multiple-select-field" : "multiple-select-field"}>
                {
                    this.props.data &&
                    <MultipleSelect id={this.props.id}
                        dataSource={this.props.data}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        optionAllLabel={"All"}
                        noneSelectedLabel={this.props.noneSelectedLabel ? this.props.noneSelectedLabel : '---Please select---'}
                        onChange={(currentItem, selectedItemsKey) => this.onChange(selectedItemsKey)} />
                }
                
                {this.props.isSummited && !this.isValid() && <FormFeedback>This field is required</FormFeedback>}
            </div>
        )
    }
}
