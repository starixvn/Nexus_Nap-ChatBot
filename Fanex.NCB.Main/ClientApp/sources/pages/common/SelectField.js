import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import Labels from '../common/Labels';

import './SelectField.scss';

export class SelectField extends Component {
    constructor(props) {
        super(props);
    }

    renderOption = (item) => <option key={item.id} value={item.id} >{item.name}</option>

    render() {
        return (
            <div className="select-field">
                <Input
                    type="select"
                    value={this.props.value}
                    name={this.props.name}
                    onChange={this.props.onChange}
                    invalid={this.props.invalid}
                >
                    <option value="" />
                    {
                        this.props.data &&
                        this.props.data.map(this.renderOption)
                    }
                </Input>
                {this.props.invalid && <FormFeedback>{Labels.FieldRequired}</FormFeedback>}
            </div>
        );
    }
}