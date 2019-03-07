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
            <div className={this.props.isSummited && !this.isValid() ? "invalid" : ""}>
                {
                    this.props.data &&
                    <MultipleSelect id="StatusList"
                        dataSource={this.props.data}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        optionAllLabel={"All"}
                        noneSelectedLabel={"---Please select---"}
                        onChange={(currentItem, selectedItemsKey) => this.onChange(selectedItemsKey)} />
                }
                
                {this.props.isSummited && !this.isValid() && <FormFeedback>This field is required</FormFeedback>}
            </div>
        )
    }
}
