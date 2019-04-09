import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }
    
    render() {        

        return (
            <div className="table-container-div">
              
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { gridParams, gridData } = state;
    return Object.assign({}, { gridParams, gridData });
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
