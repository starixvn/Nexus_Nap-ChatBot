jest.mock('react-redux');
jest.mock('redux');
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';

describe('connect', () => {
    let mapStateToProps, mapDispatchToProps;
    const connectMock = jest.fn();

    beforeAll(() => {
        connect.mockImplementation((mapStateToPropsFunc, mapDispatchToPropsFunc) => {
            mapStateToProps = mapStateToPropsFunc;
            mapDispatchToProps = mapDispatchToPropsFunc;
            return connectMock;
        });
        require('../DataGrid').default;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('props should be mapped correctly', () => {
        // Arrange
        const expected = {
            selected: [
                { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
            ],
            currentMessages: null,
            newMessages: null,
            missingMessageCsd: null,
            missingMessages: null
        };
        const state = {
            selected: [
                { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
            ],
            currentMessages: null,
            newMessages: null,
            missingMessageCsd: null,
            missingMessages: null
        };

        // Act
        const actual = mapStateToProps(state);

        // Assert
        expect(actual).toEqual(expected);
    });

    it('bindActionCreators should be called with correct parameter', () => {
        // Arrange
        const mockBindAction = jest.fn();
        bindActionCreators.mockImplementationOnce(mockBindAction);
        const dispatch = {};

        // Act
        mapDispatchToProps(dispatch);

        // Assert
        expect(mockBindAction).toHaveBeenCalledWith(ActionCreators, dispatch);
    });
});