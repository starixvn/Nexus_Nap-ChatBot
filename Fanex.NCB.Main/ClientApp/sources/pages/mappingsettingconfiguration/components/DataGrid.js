import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={3} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={15} header>{Labels.NotificationType}</Cell>
                <Cell colWidth={15} header>{Labels.NotificationType}</Cell>
                <Cell colWidth={15} header>{Labels.System}</Cell>
                <Cell colWidth={15} header>{Labels.RecipientType}</Cell>
                <Cell colWidth={15} header>{Labels.MsgContentsType}</Cell>
                <Cell colWidth={15} header>{Labels.RequiedParams}</Cell>
                <Cell colWidth={5} header>{Labels.IsTest}</Cell>
                {
                    isHost && <Cell colWidth={7} header>{Labels.Action}</Cell>
                }
            </Row>);
    };

    buildBody = isHost => {
        const { pageIndex, pageSize } = this.props.gridParams;

        this.startIndex = (pageIndex - 1) * pageSize + 1;

        if (this.props.gridData.data.length > 0) {
            return this.props.gridData.data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row>];
    }

    handleEditSettingType = e => {
        const type = parseInt(e.target.dataset.type, 10);
        this.props.getSettingType(type);
    }

    handleDeleteSettingType = e => {
        const type = parseInt(e.target.dataset.type, 10);
        this.props.openDeleteSettingTypePopup({ type, name: e.target.dataset.name });
    }

    renderRow = (item, index, isHost) => (
        <Row key={item.type} >
            <Cell className="text-center">{index + this.startIndex}</Cell>
            <Cell>{item.type}</Cell>
            <Cell>{item.name}</Cell>
            {
                isHost &&
                <Cell className="text-center">
                    <>
                        <span className="icon-edit" data-type={item.type} onClick={this.handleEditSettingType} />
                        {
                            item.unRemovable ?
                            <span className="icon-delete icon-disabled" /> :
                            <span className="icon-delete" data-type={item.type} data-name={item.name} onClick={this.handleDeleteSettingType} />
                        }
                    </>
                </Cell>
            }
        </Row>
    )

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { total } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;
        const { haveHostPermission } = this.props;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: total,
            PageList: [10, 20, 50, 100, 500]
        };

        return (
            <Table
                width={600}
                autoWidth={false}
                header={this.buildHeader(haveHostPermission)}
                body={this.buildBody(haveHostPermission)}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData } = state;
    return {
        gridParams,
        gridData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);