import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import Labels from '../../common/Labels';

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

    buildHeader = () => {
        return (
            <Row style={{ height: 32 }}>
                <Cell colWidth={30} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={100} header>{Labels.NotificationType}</Cell>
                <Cell colWidth={200} header>{Labels.TemplateName}</Cell>
                <Cell colWidth={600} header className="has-icons">{Labels.TitleContent} <span className="icon-expand" /></Cell>
                <Cell colWidth={100} header>{Labels.Action}</Cell>
            </Row>);
    };

    buildBody = (data, pageIndex, pageSize) => {
        const rows = [];
        const startIndex = (pageIndex - 1) * pageSize + 1;
        data.map((item, index) => {
            rows.push(<Row key={`body${index}`} >
                <Cell className="text-center">{index + startIndex}</Cell>
                <Cell>{item.notificationTypeName}</Cell>
                <Cell>{item.templateName}</Cell>
                <Cell>{item.title}</Cell>
                <Cell className="text-center col-action">
                    <div>
                        <span className="icon-edit" id={'edit-' + index} onClick={() => this.openEditPopup(item, index)} />
                        <UncontrolledTooltip placement="top" target={"edit-" + index}>{Labels.Edit}</UncontrolledTooltip>
                        <span className="icon-delete" id={'delete-' + index} onClick={() => this.openDeletePopup(index, item.templateId, item.templateName)} />
                        <UncontrolledTooltip placement="top" target={'delete-' + index}>Delete</UncontrolledTooltip>
                    </div>
                </Cell>
            </Row>);
        });

        return rows;
    }

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { data, count } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: count,
            PageList: [5, 10, 20, 50, 100, 500]
        };

        return (
            <div className="table-container-div">
                <Table
                    width={1400}
                    autoWidth={false}
                    header={this.buildHeader()}
                    body={this.buildBody(data, pageIndex, pageSize)}
                    isPaging
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                />
            </div>
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
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
