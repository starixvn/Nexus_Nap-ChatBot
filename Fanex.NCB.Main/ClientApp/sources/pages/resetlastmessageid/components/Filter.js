import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Form, Label } from 'reactstrap';
import Labels from '../../common/Labels';
import { MultipleSelectField } from '../../common/multiple-select-field/MultipleSelectField';

export class Filter extends Component {

    constructor(props) {
        super(props);

        this.state = {
            systemFilter: []
        };
    }

    handleChange = item => {
        const filter = item.value.split(',').map(item => parseInt(item, 10));
        this.setState({ systemFilter: filter });
    };

    handleSubmit = event => {
        event.preventDefault();
        const { systemFilter } = this.state;
        const params = this.props.selected.map(item => {
            const element = {...item};
            if (!systemFilter.includes(item.id)) {
                element.checked = false;
            } else {
                element.checked = true;
            }
            return element;
        });
        systemFilter.length > 0 && this.props.setSelected(params);
    };

    render() {
        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <div className="form-row group-form">
                    <div className="label">
                        <Label className="form-label">{Labels.Service}</Label>
                    </div>
                    <MultipleSelectField
                        data={this.props.selected}
                        keyField='id' valueField='name'
                        statusField='checked'
                        hasAllOption={this.props.selected.length > 0}
                        isAllTextShown={this.props.selected.length > 0}
                        onChange={value => this.handleChange(value)}
                    />
                    <button id="submit-button" className="btn-submit" type="submit">Submit</button>
                </div>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { selected } = state;
    return { selected };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);
