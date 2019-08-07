import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';

import './TextField.scss';

export class TextField extends Component {
    constructor(props) {
        super(props);

        this.state = {
            value: props.initialValue
        };
    }

    onChange = e => {
        this.setState(
            {
                value: e.target.value
            },
            () => {
                this.props.onChange({
                    value: this.state.value,
                    isValid: this.isValid()
                });
            }
        );
    }

    isValid = () => this.state.value !== '';

    isShowInvalid = () => {
        this.props.required && this.props.isSummited && !this.isValid();
    }

    render() {
        return (
            <div className="text-field">
                <Input type="textarea"
                    value={this.state.value}
                    onChange={this.onChange}
                    invalid={this.isShowInvalid()}
                    maxLength={this.props.maxLength}
                    placeholder={this.props.placeholder}
                    disabled={this.props.disabled}
                />
                {this.isShowInvalid() && <FormFeedback>This field is required.</FormFeedback>}
            </div>
        );
    }
}